Imports System.Runtime.Versioning
Imports System.Drawing.Drawing2D

<SupportedOSPlatform("windows")>
Public Class Form1
    ' Class-level variables
    Private missiles As List(Of SurfaceMissile)
    Private jets As List(Of Jet)
    Private WithEvents gameTimer As Timer
    Private score As Integer
    Private playerX As Integer
    Private Const targetScore As Integer = 20

    Public Sub New()
        InitializeComponent()
        ' Allow the form to receive key events before child controls.
        Me.KeyPreview = True
        ' Enable double buffering to reduce flickering
        Me.DoubleBuffered = True
        ' Set the form size to fit the backdrop
        Me.ClientSize = New Size(800, 400)
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

        ' Draw the sky
        e.Graphics.Clear(Color.LightBlue)

        ' Draw the mountains
        Dim mountainPath As New GraphicsPath()
        mountainPath.AddLines(New PointF() {
            New PointF(0, 300),
            New PointF(100, 200),
            New PointF(200, 300),
            New PointF(300, 150),
            New PointF(400, 300),
            New PointF(500, 200),
            New PointF(600, 300),
            New PointF(700, 150),
            New PointF(800, 300),
            New PointF(800, 400),
            New PointF(0, 400)
        })
        e.Graphics.FillPath(Brushes.Tan, mountainPath)

        ' Draw missiles
        For Each missile In missiles
            missile.Draw(e.Graphics)
        Next

        ' Draw jets
        For Each jet In jets
            jet.Draw(e.Graphics)
        Next

        ' Draw player
        ' Ensure the player icon is drawn within the visible area and is not obscured
        e.Graphics.FillRectangle(Brushes.Black, playerX, 380, 20, 20) ' Adjusted Y coordinate to 380

        ' Draw score
        e.Graphics.DrawString("Score: " & score, New Font("Arial", 16), Brushes.Black, 10, 10)
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Space Then
            LaunchMissile()
        ElseIf e.KeyCode = Keys.Left Then
            playerX -= 10
        ElseIf e.KeyCode = Keys.Right Then
            playerX += 10
        End If
    End Sub

    Private Sub LaunchMissile()
        ' Aim missile at the first jet in the list.
        If jets.Count > 0 Then
            Dim targetJet As Jet = jets(0)
            Dim dx As Single = targetJet.X - (playerX + 5)
            Dim dy As Single = targetJet.Y - 400  ' ground level is 400
            Dim angleRadians As Double = Math.Atan2(-dy, dx) ' Negative dy for coordinate system
            Dim angleDegrees As Single = CSng(angleRadians * 180.0 / Math.PI)

            Dim missile As New SurfaceMissile(playerX + 5, angleDegrees)
            missile.Launch()
            missiles.Add(missile)
        End If
    End Sub
End Class


