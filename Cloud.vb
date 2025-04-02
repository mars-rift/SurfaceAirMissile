Public Class Cloud
    Public X As Integer
    Public Y As Integer
    Public Width As Integer
    Public Height As Integer
    Public Speed As Single
    Public Opacity As Single

    Private Shared ReadOnly random As New Random()

    Public Sub New(screenWidth As Integer, screenHeight As Integer)
        ' Start off-screen to the right
        X = screenWidth
        ' Random Y position in the upper half of the screen
        Y = random.Next(10, screenHeight \ 3)
        ' Random size
        Width = random.Next(60, 200)
        Height = random.Next(30, 100)
        ' Random speed
        Speed = random.Next(1, 5)
        ' Random opacity
        Opacity = CSng(random.NextDouble() * 0.5 + 0.3) ' Between 0.3 and 0.8
    End Sub
    
    Public Sub Update()
        X -= CInt(Speed)
    End Sub
    
    Public Sub Draw(g As Graphics)
        ' Create a semi-transparent white brush for the cloud
        Using cloudBrush As New SolidBrush(Color.FromArgb(CInt(Opacity * 255), 255, 255, 255))
            ' Create the main cloud body
            g.FillEllipse(cloudBrush, X, Y, Width, Height)
            ' Add some smaller circles to create cloud puffs
            g.FillEllipse(cloudBrush, X - Width \ 4, Y + Height \ 4, Width \ 2, Height \ 2)
            g.FillEllipse(cloudBrush, X + Width \ 3, Y + Height \ 5, Width \ 2, Height \ 2)
            g.FillEllipse(cloudBrush, X + Width \ 2, Y + Height \ 3, Width \ 2, Height \ 2)
        End Using
    End Sub
    
    Public Function IsOffScreen() As Boolean
        Return X < -Width
    End Function
End Class