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

    Public Sub Draw(g As Graphics)
        g.FillRectangle(color, X, Y, 50, 20)
    End Sub

    Public ReadOnly Property Bounds As Rectangle
        Get
            Return New Rectangle(X, Y, 50, 20)
        End Get
    End Property

    Public Sub ResetPosition()
        X = 0
        Y = random.Next(0, 300) ' Random Y position
    End Sub
End Class
