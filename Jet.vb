Imports System.Drawing.Drawing2D
Imports System.Runtime.Versioning

<SupportedOSPlatform("windows")>
Public Class Jet
    Public Property X As Integer
    Public Property Y As Integer
    Public Property Speed As Integer
    Private random As Random
    Private color As Brush
    Private jetType As Integer ' To determine which jet shape to draw

    Public Sub New(jetColor As Brush, jetSpeed As Integer)
        random = New Random()
        color = jetColor
        Speed = jetSpeed
        jetType = random.Next(0, 3) ' Choose a random jet type (0-2)
        ResetPosition()
    End Sub

    Public Sub Update()
        ' Simple horizontal movement
        X += Speed

        ' Reset position when jet moves off-screen
        If X > 1200 Then
            ResetPosition()
        End If
    End Sub

    Public Sub Draw(g As Graphics)
        g.SmoothingMode = SmoothingMode.AntiAlias

        ' Draw different jet shapes based on jetType
        Select Case jetType
            Case 0 ' Fighter jet shape
                DrawFighterJet(g)
            Case 1 ' Bomber jet shape
                DrawBomberJet(g)
            Case 2 ' Stealth jet shape
                DrawStealthJet(g)
        End Select
    End Sub

    Private Sub DrawFighterJet(g As Graphics)
        ' Draw a fighter jet shape (single polygon)
        Dim jetPath As New GraphicsPath()

        ' Create a fighter jet shape
        jetPath.AddPolygon({
            New Point(X, Y),                      ' Nose
            New Point(X - 20, Y - 10),            ' Top of cockpit
            New Point(X - 40, Y - 10),            ' Top rear
            New Point(X - 50, Y - 25),            ' Vertical stabilizer
            New Point(X - 60, Y - 10),            ' Back top
            New Point(X - 60, Y + 10),            ' Back bottom
            New Point(X - 40, Y + 15),            ' Wing back edge
            New Point(X - 30, Y + 25),            ' Wing tip
            New Point(X - 20, Y + 10)             ' Front bottom
        })

        ' Fill the jet shape
        g.FillPath(color, jetPath)

        ' Draw exhaust flames
        Dim flamePath As New GraphicsPath()
        flamePath.AddPolygon({
            New Point(X - 60, Y),
            New Point(X - 75, Y - 8),
            New Point(X - 75, Y + 8)
        })
        g.FillPath(Brushes.OrangeRed, flamePath)
    End Sub

    Private Sub DrawBomberJet(g As Graphics)
        ' Draw a bomber jet shape (single polygon)
        Dim jetPath As New GraphicsPath()

        ' Create a bomber jet shape - wider with longer wings
        jetPath.AddPolygon({
            New Point(X, Y),                      ' Nose
            New Point(X - 30, Y - 8),             ' Top of cockpit
            New Point(X - 60, Y - 8),             ' Top rear
            New Point(X - 70, Y),                 ' Back top
            New Point(X - 60, Y + 8),             ' Back bottom
            New Point(X - 50, Y + 8),             ' Wing back edge start
            New Point(X - 45, Y + 30),            ' Wing tip
            New Point(X - 35, Y + 8),             ' Wing front edge
            New Point(X - 20, Y + 8)              ' Front bottom
        })

        ' Fill the jet shape
        g.FillPath(color, jetPath)

        ' Draw exhaust flames
        Dim flamePath As New GraphicsPath()
        flamePath.AddPolygon({
            New Point(X - 70, Y),
            New Point(X - 85, Y - 5),
            New Point(X - 85, Y + 5)
        })
        g.FillPath(Brushes.OrangeRed, flamePath)
    End Sub

    Private Sub DrawStealthJet(g As Graphics)
        ' Draw a stealth jet shape (single polygon with sharp angles)
        Dim jetPath As New GraphicsPath()

        ' Create a stealth jet shape
        jetPath.AddPolygon({
            New Point(X, Y),                      ' Nose
            New Point(X - 15, Y - 5),             ' Top front
            New Point(X - 40, Y - 5),             ' Top rear
            New Point(X - 60, Y),                 ' Back top
            New Point(X - 40, Y + 5),             ' Bottom rear
            New Point(X - 35, Y + 15),            ' Wing back edge
            New Point(X - 25, Y + 15),            ' Wing front edge
            New Point(X - 15, Y + 5)              ' Bottom front
        })

        ' Fill the jet shape
        g.FillPath(color, jetPath)

        ' Draw exhaust flames
        Dim flamePath As New GraphicsPath()
        flamePath.AddPolygon({
            New Point(X - 60, Y),
            New Point(X - 70, Y - 3),
            New Point(X - 70, Y + 3)
        })
        g.FillPath(Brushes.OrangeRed, flamePath)
    End Sub

    Public ReadOnly Property Bounds As Rectangle
        Get
            ' Create a bounding rectangle for collision detection
            Return New Rectangle(X - 60, Y - 25, 80, 50)
        End Get
    End Property

    Public Sub ResetPosition()
        X = 0  ' Start from the left edge
        Y = random.Next(50, 250)  ' Random height, but not too close to ground
    End Sub
End Class
