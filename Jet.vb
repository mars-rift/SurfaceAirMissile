Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.Versioning

<SupportedOSPlatform("windows")>
Public Class Jet
    Public Property X As Integer
    Public Property Y As Integer
    Public Property Speed As Integer
    Private random As Random
    Private color As Brush

    Public Sub New(jetColor As Brush, jetSpeed As Integer)
        random = New Random()
        color = jetColor
        Speed = jetSpeed
        ResetPosition()
    End Sub

    Public Sub Update()
        X += Speed
        If X > 800 Then ' Assuming the form width is 800
            ResetPosition()
        End If
    End Sub

    <SupportedOSPlatform("windows")>
    Public Sub Draw(g As Graphics)
        g.SmoothingMode = SmoothingMode.AntiAlias

        ' Draw the jet shape
        Dim jetPath As New GraphicsPath()
        Dim jetLength As Single = 100 ' Increased length
        Dim jetWidth As Single = 40 ' Increased width

        ' Define the jet shape (a more detailed airplane-like shape)
        jetPath.AddPolygon({
            New PointF(X, Y), ' Nose
            New PointF(X - jetLength / 2, Y + jetWidth / 2), ' Left wing
            New PointF(X - jetLength / 4, Y + jetWidth / 4), ' Left body
            New PointF(X - jetLength / 4, Y - jetWidth / 4), ' Right body
            New PointF(X - jetLength / 2, Y - jetWidth / 2) ' Right wing
        })

        ' Fill the jet shape
        g.FillPath(color, jetPath)

        ' Draw exhaust flames
        Dim flamePath As New GraphicsPath()
        flamePath.AddPolygon({
            New PointF(X - jetLength / 2, Y),
            New PointF(X - jetLength / 2 - 20, Y - 10),
            New PointF(X - jetLength / 2 - 20, Y + 10)
        })
        g.FillPath(Brushes.OrangeRed, flamePath)
    End Sub

    Public ReadOnly Property Bounds As Rectangle
        Get
            Return New Rectangle(X - 50, Y - 20, 100, 40) ' Adjust bounds to match the jet shape
        End Get
    End Property

    Public Sub ResetPosition()
        X = 0
        Y = random.Next(0, 300) ' Random Y position
    End Sub
End Class
