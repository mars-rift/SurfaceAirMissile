Imports System.Drawing.Drawing2D
Imports System.Runtime.Versioning
Imports SurfaceAirMissile

<SupportedOSPlatform("windows")>
Public Class Form1
    ' Class-level variables
    Private missiles As List(Of SurfaceMissile)
    Private jets As List(Of Jet)
    Private explosions As New List(Of Explosion)()
    Private WithEvents gameTimer As Timer
    Private score As Integer
    Private playerX As Integer
    Private Const targetScore As Integer = 100
    Private difficultyLevel As Integer
    Private previousScore As Integer

    ' Increased window dimensions
    Private Const WindowWidth As Integer = 1200
    Private Const WindowHeight As Integer = 600

    ' Background and cloud variables
    Private backgroundLayers As List(Of BackgroundLayer)
    Private clouds As List(Of Cloud)
    Private cloudTimer As Integer = 0
    Private Const CloudSpawnRate As Integer = 200 ' Lower means more clouds

    Private Enum GameState
        MainMenu
        Playing
        Paused
        GameOver
    End Enum

    Private currentState As GameState = GameState.MainMenu

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
        explosions = New List(Of Explosion)()

        ' Initialize background layers
        InitializeBackgrounds()

        ' Initialize clouds
        clouds = New List(Of Cloud)()
        ' Start with a few clouds
        For i As Integer = 0 To 5
            clouds.Add(New Cloud(WindowWidth, WindowHeight))
        Next

        gameTimer = New Timer()
        gameTimer.Interval = 50 ' Update every 50ms
        gameTimer.Start()

        score = 0
        playerX = 100
        difficultyLevel = 1

        AddJetsForCurrentLevel()
    End Sub

    Private Sub InitializeBackgrounds()
        backgroundLayers = New List(Of BackgroundLayer)()

        ' Create background images - you'll need to create these bitmap images
        ' or use resource files embedded in your project

        ' Distant mountains - slowest moving
        Dim farMountains As New Bitmap(WindowWidth, 200)
        Using g As Graphics = Graphics.FromImage(farMountains)
            g.Clear(Color.FromArgb(120, 120, 150)) ' Bluish distant mountains
            ' Create far mountains with a more complex shape
            Dim path As New GraphicsPath()
            path.AddLines(CreateMountainPoints(farMountains.Width, farMountains.Height, 5))
            g.FillPath(Brushes.SlateBlue, path)
        End Using
        backgroundLayers.Add(New BackgroundLayer(farMountains, 0.5F))

        ' Mid mountains - medium movement
        Dim midMountains As New Bitmap(WindowWidth, 150)
        Using g As Graphics = Graphics.FromImage(midMountains)
            g.Clear(Color.Transparent)
            Dim path As New GraphicsPath()
            path.AddLines(CreateMountainPoints(midMountains.Width, midMountains.Height, 8))
            Using brush As New LinearGradientBrush(
                New Rectangle(0, 0, midMountains.Width, midMountains.Height),
                Color.DarkSlateGray, Color.SlateGray, LinearGradientMode.Vertical)
                g.FillPath(brush, path)
            End Using
        End Using
        backgroundLayers.Add(New BackgroundLayer(midMountains, 1.0F))

        ' Near mountains - fastest moving
        Dim nearMountains As New Bitmap(WindowWidth, 100)
        Using g As Graphics = Graphics.FromImage(nearMountains)
            g.Clear(Color.Transparent)
            Dim path As New GraphicsPath()
            path.AddLines(CreateMountainPoints(nearMountains.Width, nearMountains.Height, 12))
            Using brush As New LinearGradientBrush(
                New Rectangle(0, 0, nearMountains.Width, nearMountains.Height),
                Color.SaddleBrown, Color.Sienna, LinearGradientMode.Vertical)
                g.FillPath(brush, path)
            End Using
        End Using
        backgroundLayers.Add(New BackgroundLayer(nearMountains, 1.5F))
    End Sub

    Private Function CreateMountainPoints(width As Integer, height As Integer, peaks As Integer) As PointF()
        Dim points As New List(Of PointF)()
        Dim random As New Random()

        ' Start at bottom left
        points.Add(New PointF(0, height))

        ' Calculate points for each peak
        Dim peakWidth As Single = width / peaks
        For i As Integer = 0 To peaks
            Dim x As Single = i * peakWidth
            Dim y As Single = If(i = 0 Or i = peaks, height, random.Next(height \ 3, height - 10))
            points.Add(New PointF(x, y))

            ' Add a point between peaks for variation
            If i < peaks Then
                points.Add(New PointF(x + peakWidth / 2, random.Next(CInt(height * 0.7), height - 5)))
            End If
        Next

        ' End at bottom right
        points.Add(New PointF(width, height))

        Return points.ToArray()
    End Function

    Private Sub AddJetsForCurrentLevel()
        jets.Clear()
        ' Add jets based on difficulty level
        For i As Integer = 1 To difficultyLevel + 2
            ' Speed increases with difficulty
            Dim speed As Integer = 10 + (difficultyLevel * 2)

            ' Alternate colors
            Dim jetColor As Brush = If(i Mod 3 = 0, Brushes.Red, If(i Mod 3 = 1, Brushes.Blue, Brushes.Green))

            jets.Add(New Jet(jetColor, speed))
        Next
    End Sub

    Private Sub gameTimer_Tick(sender As Object, e As EventArgs) Handles gameTimer.Tick
        ' Update background layers
        For Each layer In backgroundLayers
            layer.Update()
        Next

        ' Update existing clouds
        Dim cloudsToRemove As New List(Of Cloud)()
        For Each cloud In clouds
            cloud.Update()
            If cloud.IsOffScreen() Then
                cloudsToRemove.Add(cloud)
            End If
        Next

        ' Remove off-screen clouds
        For Each cloud In cloudsToRemove
            clouds.Remove(cloud)
        Next

        ' Possibly spawn new clouds
        cloudTimer += 1
        If cloudTimer >= CloudSpawnRate Then
            cloudTimer = 0
            If clouds.Count < 15 Then ' Limit maximum clouds
                clouds.Add(New Cloud(WindowWidth, WindowHeight))
            End If
        End If

        ' Update missiles
        For Each missile In missiles
            missile.Update()
        Next

        ' Update jets
        For Each jet In jets
            jet.Update()
        Next

        ' Update explosions and check collisions
        CheckCollision()
        Me.Invalidate() ' Redraw the form
    End Sub

    Private Sub CheckCollision()
        Dim missilesToRemove As New List(Of SurfaceMissile)()

        For Each missile In missiles
            If missile.IsLaunched Then
                For Each jet In jets
                    ' Improved collision detection with smaller hitbox
                    If CalculatePreciseCollision(missile, jet) Then
                        ' Add explosion effect
                        explosions.Add(New Explosion(jet.X, jet.Y))

                        missilesToRemove.Add(missile)
                        score += 1
                        jet.ResetPosition()

                        ' Play sound if you add sound support
                        ' PlaySound("explosion.wav")

                        If score >= targetScore Then
                            GameOver()
                        End If

                        Exit For ' Don't check other jets for this missile
                    End If
                Next
            End If
        Next

        ' Remove hit missiles
        For Each missile In missilesToRemove
            missiles.Remove(missile)
        Next

        ' Update explosions and remove finished ones
        Dim explosionsToRemove As New List(Of Explosion)()
        For Each explosion In explosions
            explosion.Update()
            If explosion.IsFinished Then
                explosionsToRemove.Add(explosion)
            End If
        Next

        For Each explosion In explosionsToRemove
            explosions.Remove(explosion)
        Next

        ' Check for difficulty increase
        If score Mod 5 = 0 AndAlso score > 0 AndAlso previousScore <> score Then
            previousScore = score
            difficultyLevel += 1
            AddJetsForCurrentLevel()
        End If
    End Sub

    Private Function CalculatePreciseCollision(missile As SurfaceMissile, jet As Jet) As Boolean
        ' More precise collision than just bounding box
        Dim missileCenter As New Point(missile.X, missile.Y)
        Dim jetCenter As New Point(jet.X + jet.Width / 2, jet.Y + jet.Height / 2)

        ' Calculate distance between centers
        Dim distance As Double = Math.Sqrt((missileCenter.X - jetCenter.X) ^ 2 +
                                            (missileCenter.Y - jetCenter.Y) ^ 2)

        ' Collision occurs if distance is less than sum of half-widths
        Return distance < (missile.Width / 2 + jet.Width / 2)
    End Function

    Private Sub GameOver()
        currentState = GameState.GameOver
        gameTimer.Stop()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Select Case currentState
            Case GameState.MainMenu
                DrawMainMenu(e.Graphics)
            Case GameState.Playing
                DrawGameplay(e.Graphics)
            Case GameState.Paused
                DrawGameplay(e.Graphics)
                DrawPausedOverlay(e.Graphics)
            Case GameState.GameOver
                DrawGameplay(e.Graphics)
                DrawGameOverScreen(e.Graphics)
        End Select
    End Sub

    Private Sub DrawMainMenu(g As Graphics)
        g.Clear(Color.DarkRed)
        g.DrawString("SURFACE TO AIR MISSILE GAME", New Font("Arial", 24, FontStyle.Bold), Brushes.Black, WindowWidth / 2 - 250, 100)
        g.DrawString("Press Enter to Start", New Font("Arial", 18), Brushes.Black, WindowWidth / 2 - 100, 200)
        g.DrawString("Arrow Keys to Move, Space,X,Z to Fire", New Font("Arial", 14), Brushes.Black, WindowWidth / 2 - 150, 250)
    End Sub

    Private Sub DrawGameplay(g As Graphics)
        ' Enable high quality rendering
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.InterpolationMode = InterpolationMode.HighQualityBicubic

        ' Draw the sky with gradient
        Using skyBrush As New LinearGradientBrush(
            New Rectangle(0, 0, WindowWidth, WindowHeight),
            Color.FromArgb(135, 206, 250), ' LightSkyBlue
            Color.FromArgb(65, 105, 225),  ' RoyalBlue
            LinearGradientMode.Vertical)
            g.FillRectangle(skyBrush, 0, 0, WindowWidth, WindowHeight)
        End Using

        ' Draw clouds
        For Each cloud In clouds
            cloud.Draw(g)
        Next

        ' Draw parallax background layers
        For Each layer In backgroundLayers
            layer.Draw(g, WindowHeight)
        Next

        ' Draw missiles
        For Each missile In missiles
            missile.Draw(g)
        Next

        ' Draw jets
        For Each jet In jets
            jet.Draw(g)
        Next

        ' Draw explosions
        For Each explosion In explosions
            explosion.Draw(g)
        Next

        ' Draw player platform
        Dim groundY As Integer = WindowHeight - 20
        Using platformBrush As New LinearGradientBrush(
            New Rectangle(playerX - 20, groundY, 60, 20),
            Color.DarkGray, Color.LightGray, LinearGradientMode.Vertical)
            g.FillRectangle(platformBrush, playerX - 20, groundY, 60, 20)
        End Using

        ' Draw player launcher - more detailed
        Using launcherBrush As New SolidBrush(Color.DarkGreen)
            ' Base
            g.FillRectangle(launcherBrush, playerX - 5, groundY - 20, 30, 20)
            ' Turret
            g.FillRectangle(launcherBrush, playerX + 5, groundY - 30, 10, 20)
        End Using

        ' Draw score with nicer font and shadow for visibility
        Dim scoreText As String = "Score: " & score
        Dim scoreFont As New Font("Arial", 16, FontStyle.Bold)

        ' Draw shadow first
        g.DrawString(scoreText, scoreFont, Brushes.Black, 12, 12)
        ' Draw text on top
        g.DrawString(scoreText, scoreFont, Brushes.White, 10, 10)

        ' Draw difficulty level
        Dim levelText As String = "Level: " & difficultyLevel
        g.DrawString(levelText, scoreFont, Brushes.Black, 12, 42)
        g.DrawString(levelText, scoreFont, Brushes.White, 10, 40)
    End Sub

    Private Sub DrawPausedOverlay(g As Graphics)
        ' Semi-transparent overlay
        Using brush As New SolidBrush(Color.FromArgb(128, 0, 0, 0))
            g.FillRectangle(brush, 0, 0, WindowWidth, WindowHeight)
        End Using
        g.DrawString("PAUSED", New Font("Arial", 36, FontStyle.Bold), Brushes.White, WindowWidth / 2 - 100, WindowHeight / 2 - 50)
        g.DrawString("Press P to Resume", New Font("Arial", 18), Brushes.White, WindowWidth / 2 - 100, WindowHeight / 2 + 20)
    End Sub

    Private Sub DrawGameOverScreen(g As Graphics)
        Using brush As New SolidBrush(Color.FromArgb(160, 0, 0, 0))
            g.FillRectangle(brush, 0, 0, WindowWidth, WindowHeight)
        End Using
        g.DrawString("GAME OVER", New Font("Arial", 36, FontStyle.Bold), Brushes.White, WindowWidth / 2 - 140, WindowHeight / 2 - 80)
        g.DrawString("Final Score: " & score, New Font("Arial", 24), Brushes.White, WindowWidth / 2 - 100, WindowHeight / 2 - 20)
        g.DrawString("Press Enter to Restart", New Font("Arial", 18), Brushes.White, WindowWidth / 2 - 110, WindowHeight / 2 + 40)
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case currentState
            Case GameState.MainMenu
                If e.KeyCode = Keys.Enter Then
                    currentState = GameState.Playing
                    InitializeGame()
                End If
            Case GameState.Playing
                If e.KeyCode = Keys.Space Then
                    LaunchMissile(SurfaceAirMissile.MissileType.Standard)
                ElseIf e.KeyCode = Keys.Z Then
                    LaunchMissile(SurfaceAirMissile.MissileType.Fast)
                ElseIf e.KeyCode = Keys.X Then
                    LaunchMissile(SurfaceAirMissile.MissileType.Guided)
                ElseIf e.KeyCode = Keys.Left Then
                    playerX -= 10
                    If playerX < 0 Then playerX = 0
                ElseIf e.KeyCode = Keys.Right Then
                    playerX += 10
                    If playerX > WindowWidth - 20 Then playerX = WindowWidth - 20
                ElseIf e.KeyCode = Keys.P Then
                    currentState = GameState.Paused
                    gameTimer.Stop()
                End If
            Case GameState.Paused
                If e.KeyCode = Keys.P Then
                    currentState = GameState.Playing
                    gameTimer.Start()
                End If
            Case GameState.GameOver
                If e.KeyCode = Keys.Enter Then
                    currentState = GameState.Playing
                    InitializeGame()
                End If
        End Select
    End Sub

    Private Sub LaunchMissile(Optional missileType As SurfaceAirMissile.MissileType = SurfaceAirMissile.MissileType.Standard)
        If jets.Count > 0 Then
            Dim playerY As Integer = WindowHeight - 40 ' Launcher position

            ' Get nearest jet
            Dim nearestJet As Jet = GetNearestJet()
            Dim dx As Single = nearestJet.X - playerX
            Dim dy As Single = nearestJet.Y - playerY
            Dim angleRadians As Double = Math.Atan2(-dy, dx)
            Dim angleDegrees As Single = CSng(angleRadians * 180.0 / Math.PI)

            ' Create missile
            Dim missile As SurfaceMissile

            If missileType = SurfaceAirMissile.MissileType.Guided Then
                missile = New SurfaceMissile(playerX, playerY, angleDegrees, nearestJet)
            Else
                missile = New SurfaceMissile(playerX, playerY, angleDegrees, missileType)
            End If

            missile.Launch()
            missiles.Add(missile)
        End If
    End Sub

    ' Helper method to find nearest jet
    Private Function GetNearestJet() As Jet
        Dim nearestJet As Jet = jets(0)
        Dim shortestDistance As Double = Double.MaxValue
        Dim playerY As Integer = WindowHeight - 40 ' Launcher position

        For Each jet In jets
            Dim distance As Double = Math.Sqrt((jet.X - playerX) ^ 2 + (jet.Y - playerY) ^ 2)
            If distance < shortestDistance Then
                shortestDistance = distance
                nearestJet = jet
            End If
        Next

        Return nearestJet
    End Function
End Class
