Imports System.Drawing.Drawing2D
Imports System.Runtime.Versioning

<SupportedOSPlatform("windows")>
Public Class Form1
    ' Class-level variables
    Private missiles As List(Of SurfaceMissile)
    Private jets As List(Of Jet)
    Private WithEvents gameTimer As Timer
    Private score As Integer
    Private playerX As Integer
    Private Const targetScore As Integer = 20

    ' Increased window dimensions
    Private Const WindowWidth As Integer = 1200
    Private Const WindowHeight As Integer = 600

    Public Sub New()
        InitializeComponent()
        ' Allow the form to receive key events before child controls.
        Me.KeyPreview = True
        ' Enable double buffering to reduce flickering
        Me.DoubleBuffered = True
        ' Set the form size to the new larger dimensions
        Me.ClientSize = New Size(WindowWidth, WindowHeight)
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
        jets.Add(New Jet(Brushes.Blue, 10)) ' Faster speed
        jets.Add(New Jet(Brushes.Green, 12)) ' Even faster speed
        jets.Add(New Jet(Brushes.Red, 14)) ' Fastest speed
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
            ' Only consider collisions for launched missiles
            If missile.IsLaunched Then
                For Each jet In jets
                    If missile.Bounds.IntersectsWith(jet.Bounds) Then
                        missile.IsLaunched = False
                        missile.Y = WindowHeight - 20 ' Reset missile position to ground level
                        score += 1
                        jet.ResetPosition()
                        If score >= targetScore Then
                            GameOver()
                        End If
                    End If
                Next
            End If
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

        ' Draw the mountains - expanded for wider window
        Dim mountainPath As New GraphicsPath()
        Dim groundY As Integer = WindowHeight - 100 ' Mountain base height

        ' Create the mountain path with more peaks for the wider window
        mountainPath.AddLines(New PointF() {
            New PointF(0, groundY),
            New PointF(100, groundY - 100),
            New PointF(200, groundY),
            New PointF(300, groundY - 150),
            New PointF(400, groundY),
            New PointF(500, groundY - 100),
            New PointF(600, groundY),
            New PointF(700, groundY - 150),
            New PointF(800, groundY),
            New PointF(900, groundY - 120),
            New PointF(1000, groundY),
            New PointF(1100, groundY - 130),
            New PointF(1200, groundY),
            New PointF(WindowWidth, WindowHeight),
            New PointF(0, WindowHeight)
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

        ' Draw player - adjusted for new ground level
        e.Graphics.FillRectangle(Brushes.Black, playerX, WindowHeight - 20, 20, 20)

        ' Draw score
        e.Graphics.DrawString("Score: " & score, New Font("Arial", 16), Brushes.Black, 10, 10)
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Space Then
            LaunchMissile()
        ElseIf e.KeyCode = Keys.Left Then
            playerX -= 10
            ' Prevent going off-screen to the left
            If playerX < 0 Then playerX = 0
        ElseIf e.KeyCode = Keys.Right Then
            playerX += 10
            ' Prevent going off-screen to the right
            If playerX > WindowWidth - 20 Then playerX = WindowWidth - 20
        End If
    End Sub

    Private Sub LaunchMissile()
        ' Aim missile at the first jet in the list.
        If jets.Count > 0 Then
            ' Define player position (center of player icon)
            Dim playerY As Integer = WindowHeight - 20 ' Same Y position used to draw player

            Dim targetJet As Jet = jets(0)
            Dim dx As Single = targetJet.X - (playerX + 10) ' Center X of player (playerX + 10)
            Dim dy As Single = targetJet.Y - playerY  ' From player Y position
            Dim angleRadians As Double = Math.Atan2(-dy, dx)
            Dim angleDegrees As Single = CSng(angleRadians * 180.0 / Math.PI)

            ' Create missile at the player's position
            Dim missile As New SurfaceMissile(playerX + 10, playerY, angleDegrees)
            missile.Launch()
            missiles.Add(missile)
        End If
    End Sub


End Class
