Public Class Form1
    Private missiles As List(Of SurfaceMissile)
    Private jets As List(Of Jet)
    Private WithEvents gameTimer As Timer
    Private score As Integer
    Private playerX As Integer
    Private Const targetScore As Integer = 10

    Public Sub New()
        InitializeComponent()
        InitializeGame()
    End Sub

    Private Sub InitializeGame()
        missiles = New List(Of SurfaceMissile)()
        jets = New List(Of Jet)()
        gameTimer = New Timer()
        gameTimer.Interval = 50 ' Update every 50ms
        gameTimer.Start()
        score = 0
        playerX = 100

        ' Add different types of jets
        jets.Add(New Jet(Brushes.Blue, 5))
        jets.Add(New Jet(Brushes.Green, 7))
        jets.Add(New Jet(Brushes.Red, 9))
    End Sub

    Private Sub gameTimer_Tick(sender As Object, e As EventArgs) Handles gameTimer.Tick
        For Each missile In missiles
            missile.Update()
        Next
        For Each jet In jets
            jet.Update()
        Next
        CheckCollision()
        Me.Invalidate() ' Redraw the form
    End Sub

    Private Sub CheckCollision()
        For Each missile In missiles
            For Each jet In jets
                If missile.Bounds.IntersectsWith(jet.Bounds) Then
                    missile.IsLaunched = False
                    missile.Y = 400 ' Reset missile position
                    score += 1
                    jet.ResetPosition()
                    If score >= targetScore Then
                        GameOver()
                    End If
                End If
            Next
        Next
    End Sub

    Private Sub GameOver()
        gameTimer.Stop()
        MessageBox.Show("Game Over! Your score is: " & score)
        Application.Exit()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        For Each missile In missiles
            missile.Draw(e.Graphics)
        Next
        For Each jet In jets
            jet.Draw(e.Graphics)
        Next
        e.Graphics.FillRectangle(Brushes.Black, playerX, 400, 20, 20) ' Draw player
        e.Graphics.DrawString("Score: " & score, New Font("Arial", 16), Brushes.Black, 10, 10)
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Space Then
            Dim missile As New SurfaceMissile(playerX + 5) ' Launch missile from player's position
            missile.Launch()
            missiles.Add(missile)
        ElseIf e.KeyCode = Keys.Left Then
            playerX -= 10 ' Move player left
        ElseIf e.KeyCode = Keys.Right Then
            playerX += 10 ' Move player right
        End If
    End Sub
End Class
