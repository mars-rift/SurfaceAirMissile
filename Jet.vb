Imports System.Drawing.Drawing2D
Imports System.Runtime.Versioning

<SupportedOSPlatform("windows")>
Public Class Jet
    Public Property X As Integer
    Public Property Y As Integer
    Public Property Speed As Integer
    Public Property JetColor As Brush

    Private ReadOnly jetWidth As Integer = 40
    Private ReadOnly jetHeight As Integer = 20
    Private ReadOnly random As New Random()

    Public Sub New(color As Brush, Optional speed As Integer = 10)
        Me.JetColor = color
        Me.Speed = speed
        ResetPosition()
    End Sub

    Public Sub Update()
        ' Move jet from right to left
        X -= Speed

        ' If jet moves off-screen, reset it
        If X < -jetWidth Then
            ResetPosition()
        End If
    End Sub

    Public Sub ResetPosition()
        ' Start jet at random height on the right side of screen
        X = 1200 + random.Next(100, 500)  ' Adjust range based on window width
        Y = random.Next(50, 300)  ' Adjust range based on desired flight heights
    End Sub

    Public Sub Draw(g As Graphics)
        ' Save the current state to restore later
        Dim state = g.Save()

        ' Draw jet body
        Dim bodyPath As New Drawing2D.GraphicsPath()
        bodyPath.AddEllipse(X, Y, jetWidth, CInt(jetHeight / 2))
        g.FillPath(JetColor, bodyPath)

        ' Draw jet cockpit (towards the front/left)
        Dim cockpitBrush As New SolidBrush(Color.LightBlue)
        g.FillEllipse(cockpitBrush, X + 5, Y + 2, 10, 6)

        ' Draw wings
        Dim wingPath As New Drawing2D.GraphicsPath()
        wingPath.AddPolygon({
            New Point(X + jetWidth / 2 - 5, Y + jetHeight / 2), ' Wing start on body
            New Point(X + jetWidth / 2 - 15, Y + jetHeight),     ' Wing tip (down)
            New Point(X + jetWidth / 2 + 15, Y + jetHeight / 2)  ' Wing end on body
        })
        g.FillPath(JetColor, wingPath)

        ' Draw tail fin
        Dim tailPath As New Drawing2D.GraphicsPath()
        tailPath.AddPolygon({
            New Point(X + jetWidth - 10, Y + 2),        ' Top of tail 
            New Point(X + jetWidth + 5, Y - 8),         ' Tip of tail
            New Point(X + jetWidth, Y + jetHeight / 4)  ' Bottom of tail
        })
        g.FillPath(JetColor, tailPath)

        ' Draw engine exhaust
        Using exhaustBrush As New LinearGradientBrush(
                New Point(X + jetWidth, Y + jetHeight / 4 - 2),
                New Point(X + jetWidth + 8, Y + jetHeight / 4 - 2),
                Color.Yellow,
                Color.Red)
            g.FillRectangle(CType(exhaustBrush, Brush), X + jetWidth, CInt(Y + jetHeight / 4 - 2), 8, 4)
        End Using

        ' Restore graphics state
        g.Restore(state)
    End Sub

    Public ReadOnly Property Width As Integer
        Get
            Return jetWidth
        End Get
    End Property

    Public ReadOnly Property Height As Integer
        Get
            Return jetHeight
        End Get
    End Property
End Class
