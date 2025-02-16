Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.Versioning

<SupportedOSPlatform("windows")>
Public Class SurfaceMissile
    ' Position and motion
    Public Property X As Single
    Public Property Y As Single
    Public Property Speed As Single
    Public Property Angle As Single  ' In degrees
    Public Property Gravity As Single
    Public Property IsLaunched As Boolean

    ' For collision size
    Private Const MissileRadius As Integer = 10

    ' Trail feature
    Private Const MaxTrailPoints As Integer = 50
    Private trailPoints As List(Of PointF)

    Public Sub New(startX As Integer, startAngle As Single)
        X = startX
        Y = 400
        Speed = 15.0F
        Angle = startAngle
        Gravity = 0.5F
        IsLaunched = False
        trailPoints = New List(Of PointF)()
    End Sub

    Public Sub Launch()
        IsLaunched = True
    End Sub

    Public Sub Update()
        If Not IsLaunched Then
            Exit Sub
        End If

        ' Convert angle (degrees) to radians
        Dim angleRad As Double = Math.PI * Angle / 180.0

        ' Update X/Y using standard projectile physics
        X += CSng(Speed * Math.Cos(angleRad))
        Y -= CSng(Speed * Math.Sin(angleRad))

        ' Gravity effect
        Y += Gravity

        ' Add current position to trail
        trailPoints.Add(New PointF(X, Y))
        If trailPoints.Count > MaxTrailPoints Then
            trailPoints.RemoveAt(0)
        End If
    End Sub

    <SupportedOSPlatform("windows")>
    Public Sub Draw(g As Graphics)
        If Not IsLaunched Then
            Return
        End If

        g.SmoothingMode = SmoothingMode.AntiAlias

        ' Draw trail
        If trailPoints.Count > 1 Then
            g.DrawLines(Pens.Gray, trailPoints.ToArray())
        End If

        ' Draw rocket-like missile
        Dim state = g.Save()
        g.TranslateTransform(X, Y)
        g.RotateTransform(-Angle)

        Dim rocketLength As Single = 20
        Dim rocketWidth As Single = 10
        Dim rocketPath As New GraphicsPath()
        rocketPath.AddPolygon({
            New PointF(0, -rocketLength / 2),    ' tip
            New PointF(rocketWidth / 2, rocketLength / 2),
            New PointF(-rocketWidth / 2, rocketLength / 2)
        })

        g.FillPath(Brushes.Red, rocketPath)
        g.Restore(state)
    End Sub

    ' Rectangle-based collision (still good for simpler checks)
    Public ReadOnly Property Bounds As Rectangle
        Get
            Return New Rectangle(
                CInt(X - MissileRadius),
                CInt(Y - MissileRadius),
                MissileRadius * 2,
                MissileRadius * 2
            )
        End Get
    End Property
End Class
