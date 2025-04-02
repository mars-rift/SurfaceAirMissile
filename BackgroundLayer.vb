Public Class BackgroundLayer
    Public X As Single
    Public Speed As Single
    Public Image As Bitmap
    Public Width As Integer
    Public Height As Integer
    
    Public Sub New(image As Bitmap, speed As Single)
        Me.Image = image
        Me.Speed = speed
        Me.X = 0
        Me.Width = image.Width
        Me.Height = image.Height
    End Sub
    
    Public Sub Update()
        ' Move the layer based on speed
        X -= Speed
        
        ' Reset position when the image scrolls off-screen
        If X <= -Width Then
            X = 0
        End If
    End Sub
    
    Public Sub Draw(g As Graphics, viewportHeight As Integer)
        ' Draw the main image
        g.DrawImage(Image, CInt(X), viewportHeight - Height, Width, Height)
        
        ' Draw a second image for seamless scrolling
        If X < 0 Then
            g.DrawImage(Image, CInt(X) + Width, viewportHeight - Height, Width, Height)
        End If
    End Sub
End Class