Public Class Explosion
    Public X As Integer
    Public Y As Integer
    Private radius As Integer = 5
    Private ReadOnly maxRadius As Integer = 30
    Private ReadOnly growthRate As Integer = 3
    Public IsFinished As Boolean = False
    
    Public Sub New(x As Integer, y As Integer)
        Me.X = x
        Me.Y = y
    End Sub
    
    Public Sub Update()
        radius += growthRate
        If radius >= maxRadius Then
            IsFinished = True
        End If
    End Sub
    
    Public Sub Draw(g As Graphics)
        ' Create gradient for explosion effect
        Using brush As New Drawing2D.LinearGradientBrush(
            New Rectangle(X - radius, Y - radius, radius * 2, radius * 2),
            Color.Yellow, Color.Red, Drawing2D.LinearGradientMode.ForwardDiagonal)

            g.FillEllipse(brush, X - radius, Y - radius, radius * 2, radius * 2)
        End Using

        ' Draw outer glow
        Using pen As New Pen(Color.FromArgb(128, Color.Orange), 2)
            g.DrawEllipse(pen, X - radius - 2, Y - radius - 2, (radius + 2) * 2, (radius + 2) * 2)
        End Using
    End Sub
End Class