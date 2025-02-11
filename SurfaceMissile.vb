Public Class SurfaceMissile
    Public Property X As Integer
    Public Property Y As Integer
    Public Property Speed As Integer
    Public Property IsLaunched As Boolean

    Public Sub New(startX As Integer)
        X = startX
        Y = 400
        Speed = 10
        IsLaunched = False
    End Sub

    Public Sub Launch()
        IsLaunched = True
    End Sub

    Public Sub Update()
        If IsLaunched Then
            Y -= Speed
        End If
    End Sub

    Public Sub Draw(g As Graphics)
        If IsLaunched Then
            g.FillRectangle(Brushes.Red, X, Y, 10, 20)
        End If
    End Sub

    Public ReadOnly Property Bounds As Rectangle
        Get
            Return New Rectangle(X, Y, 10, 20)
        End Get
    End Property
End Class
