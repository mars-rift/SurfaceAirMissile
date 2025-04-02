Imports System.Drawing.Drawing2D
Imports System.Runtime.Versioning
Imports SurfaceAirMissile

<SupportedOSPlatform("windows")>
Public Class SurfaceMissile
    ' Position and motion
    Public Property X As Single
    Public Property Y As Single
    Public Property Speed As Single
    Public Property Angle As Single  ' In degrees
    Public Property Gravity As Single
    Public Property IsLaunched As Boolean
    Public Property MissileType As SurfaceAirMissile.MissileType

    ' Previous position to calculate direction vector
    Private prevX As Single
    Private prevY As Single

    ' Cached values to avoid recalculations
    Private cachedMoveAngle As Single
    Private cachedPoints() As PointF

    ' For collision size
    Private Const MissileRadius As Integer = 10
    Private collisionBounds As Rectangle = New Rectangle(0, 0, MissileRadius * 2, MissileRadius * 2)

    ' Trail feature
    Private Const MaxTrailPoints As Integer = 50
    Private ReadOnly trailPoints As New List(Of PointF)(MaxTrailPoints)
    Private ReadOnly trailPen As New Pen(Color.Gray, 1.5F)

    ' Missile appearance
    Private ReadOnly rocketBrush As New SolidBrush(Color.Red)
    Private ReadOnly rocketPath As New GraphicsPath()

    ' Target tracking for guided missiles
    Private targetJet As Jet = Nothing

    ' Fixed constructor that includes all possible parameters
    Public Sub New(startX As Integer, startY As Integer, startAngle As Single, Optional missileType As SurfaceAirMissile.MissileType = SurfaceAirMissile.MissileType.Standard)
        X = startX
        Y = startY
        prevX = startX
        prevY = startY
        Me.MissileType = missileType

        ' Set speed based on missile type
        Select Case missileType
            Case SurfaceAirMissile.MissileType.Fast
                Speed = 25.0F
                Gravity = 0.3F
            Case SurfaceAirMissile.MissileType.Guided
                Speed = 12.0F
                Gravity = 0.2F
            Case Else ' Standard
                Speed = 15.0F
                Gravity = 0.5F
        End Select

        Angle = startAngle
        IsLaunched = False

        ' Initialize the missile shape just once
        Dim rocketLength As Single = 20
        Dim rocketWidth As Single = 10
        rocketPath.AddPolygon({
            New PointF(0, -rocketLength / 2),    ' tip
            New PointF(rocketWidth / 2, rocketLength / 2),
            New PointF(-rocketWidth / 2, rocketLength / 2)
        })
    End Sub

    ' Simpler constructor that calls the main one
    Public Sub New(startX As Integer, startAngle As Single)
        Me.New(startX, 400, startAngle, SurfaceAirMissile.MissileType.Standard)
    End Sub

    ' Constructor for missile with target (guided)
    Public Sub New(startX As Integer, startY As Integer, startAngle As Single, target As Jet)
        Me.New(startX, startY, startAngle, SurfaceAirMissile.MissileType.Guided)
        targetJet = target
    End Sub

    Public Sub Launch()
        IsLaunched = True
    End Sub

    Public Sub Update()
        If Not IsLaunched Then
            Exit Sub
        End If

        ' Store previous position
        prevX = X
        prevY = Y

        ' Handle guided missiles differently
        If MissileType = SurfaceAirMissile.MissileType.Guided AndAlso targetJet IsNot Nothing Then
            UpdateGuidedMissile()
        Else
            ' Convert angle (degrees) to radians - use a faster calculation method
            Dim angleRad As Double = Angle * 0.017453292519943295 ' This is Math.PI/180 precalculated

            ' Update X/Y using standard projectile physics
            X += CSng(Speed * Math.Cos(angleRad))
            Y -= CSng(Speed * Math.Sin(angleRad))

            ' Gravity effect
            Y += Gravity
        End If

        ' Add current position to trail
        trailPoints.Add(New PointF(X, Y))
        If trailPoints.Count > MaxTrailPoints Then
            trailPoints.RemoveAt(0)
        End If

        ' Cache movement angle for drawing
        If Math.Abs(X - prevX) < 0.001F AndAlso Math.Abs(Y - prevY) < 0.001F Then
            cachedMoveAngle = Angle
        Else
            ' Calculate angle from movement vector
            Dim dx As Single = X - prevX
            Dim dy As Single = Y - prevY
            cachedMoveAngle = CSng(Math.Atan2(-dy, dx) * 57.2957795131) ' 180/pi precalculated
        End If

        ' Update collision bounds - create a new Rectangle instead of trying to modify properties
        collisionBounds = New Rectangle(CInt(X - MissileRadius), CInt(Y - MissileRadius), MissileRadius * 2, MissileRadius * 2)

        ' Cache the trail points array to avoid conversion in Draw()
        cachedPoints = trailPoints.ToArray()
    End Sub

    Private Sub UpdateGuidedMissile()
        ' Calculate direction to target
        Dim dx As Single = targetJet.X - X
        Dim dy As Single = targetJet.Y - Y

        ' Calculate angle to target
        Dim targetAngle As Single = CSng(Math.Atan2(-dy, dx) * 57.2957795131) ' 180/pi precalculated

        ' Gradually adjust angle toward target (homing effect)
        Dim angleAdjustment As Single = 2.0F ' How quickly the missile can turn

        ' Find the shortest path to turn to the target angle
        Dim angleDiff As Single = targetAngle - Angle

        ' Normalize the angle difference to -180 to 180
        While angleDiff > 180
            angleDiff -= 360
        End While

        While angleDiff < -180
            angleDiff += 360
        End While

        ' Apply adjustment (limited by turn rate)
        If Math.Abs(angleDiff) < angleAdjustment Then
            Angle = targetAngle
        ElseIf angleDiff > 0 Then
            Angle += angleAdjustment
        Else
            Angle -= angleAdjustment
        End If

        ' Convert angle to radians
        Dim angleRad As Double = Angle * 0.017453292519943295

        ' Update position
        X += CSng(Speed * Math.Cos(angleRad))
        Y -= CSng(Speed * Math.Sin(angleRad))
    End Sub

    Public Sub Draw(g As Graphics)
        If Not IsLaunched Then
            Return
        End If

        g.SmoothingMode = SmoothingMode.AntiAlias

        ' Draw trail using cached points
        If trailPoints.Count > 1 Then
            ' Set trail color based on missile type
            Select Case MissileType
                Case SurfaceAirMissile.MissileType.Fast
                    trailPen.Color = Color.Orange
                Case SurfaceAirMissile.MissileType.Guided
                    trailPen.Color = Color.LightBlue
                Case Else
                    trailPen.Color = Color.Gray
            End Select

            g.DrawLines(trailPen, cachedPoints)
        End If

        ' Draw rocket-like missile
        Dim state = g.Save()
        g.TranslateTransform(X, Y)

        ' Use the cached movement angle
        g.RotateTransform(cachedMoveAngle - 90) ' -90 because our triangle points up by default

        ' Set missile color based on type
        Select Case MissileType
            Case SurfaceAirMissile.MissileType.Fast
                rocketBrush.Color = Color.Orange
            Case SurfaceAirMissile.MissileType.Guided
                rocketBrush.Color = Color.Blue
            Case Else
                rocketBrush.Color = Color.Red
        End Select

        ' Draw using the pre-defined path
        g.FillPath(rocketBrush, rocketPath)
        g.Restore(state)
    End Sub

    ' Helper for collision detection - exposed Width property
    Public ReadOnly Property Width As Integer
        Get
            Return MissileRadius * 2
        End Get
    End Property

    ' Rectangle-based collision (still good for simpler checks)
    Public ReadOnly Property Bounds As Rectangle
        Get
            Return collisionBounds
        End Get
    End Property
End Class
