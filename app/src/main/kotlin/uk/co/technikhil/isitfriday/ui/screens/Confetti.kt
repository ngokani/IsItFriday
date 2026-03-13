package uk.co.technikhil.isitfriday.ui.screens

import androidx.compose.animation.core.Animatable
import androidx.compose.animation.core.LinearEasing
import androidx.compose.animation.core.tween
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.drawscope.DrawScope
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlin.math.cos
import kotlin.math.sin
import kotlin.random.Random

private const val NUM_PARTICLES = 100
private const val PARTICLE_LIFESPAN_MS = 2000L // 2 seconds
private const val PARTICLE_INITIAL_SPEED_MIN = 200f
private const val PARTICLE_INITIAL_SPEED_MAX = 500f
private const val GRAVITY = 9.8f * 60f // Adjusted for per-frame updates

data class Particle(
    val id: Long = Random.nextLong(),
    var x: Float,
    var y: Float,
    var velocityX: Float,
    var velocityY: Float,
    val color: Color,
    val initialAlpha: Float = 1f,
    var currentAlpha: Float = initialAlpha,
    val size: Float = Random.nextFloat() * 10f + 5f, // Size between 5 and 15
    val creationTime: Long = System.currentTimeMillis(),
    var rotation: Float = Random.nextFloat() * 360f, // Initial rotation
    var rotationSpeed: Float = Random.nextFloat() * 20f - 10f // Rotation speed between -10 and 10 deg/frame
) {
    fun isAlive(currentTime: Long): Boolean = (currentTime - creationTime) < PARTICLE_LIFESPAN_MS

    fun update(deltaTimeS: Float, canvasWidth: Float, canvasHeight: Float) {
        // Apply gravity
        velocityY += GRAVITY * deltaTimeS

        // Update position
        x += velocityX * deltaTimeS
        y += velocityY * deltaTimeS

        // Update rotation
        rotation += rotationSpeed * deltaTimeS * 60f // Assuming 60fps for rotation speed adjustment

        // Fade out over lifespan
        val elapsedTime = System.currentTimeMillis() - creationTime
        currentAlpha = initialAlpha * (1f - (elapsedTime.toFloat() / PARTICLE_LIFESPAN_MS)).coerceAtLeast(0f)
    }
}

@Composable
fun ConfettiEffect(
    modifier: Modifier = Modifier,
    trigger: Boolean, // Change this to trigger the confetti
    onFinished: () -> Unit // Callback when animation finishes
) {
    var particles by remember { mutableStateOf<List<Particle>>(emptyList()) }
    val animationRunning = remember { mutableStateOf(false) }

    LaunchedEffect(trigger) {
        if (trigger && !animationRunning.value) {
            animationRunning.value = true
            val newParticles = mutableListOf<Particle>()
            val screenCenterX = 0f // Will be updated with actual canvas width
            val screenCenterY = 0f // Will be updated with actual canvas height

            for (i in 0 until NUM_PARTICLES) {
                val angle = Random.nextFloat() * 2 * Math.PI
                val speed = Random.nextFloat() * (PARTICLE_INITIAL_SPEED_MAX - PARTICLE_INITIAL_SPEED_MIN) + PARTICLE_INITIAL_SPEED_MIN
                newParticles.add(
                    Particle(
                        x = screenCenterX, // Initial position, will be updated to center
                        y = screenCenterY, // Initial position, will be updated to center
                        velocityX = (cos(angle) * speed).toFloat(),
                        velocityY = (sin(angle) * speed).toFloat(),
                        color = Color(
                            red = Random.nextFloat(),
                            green = Random.nextFloat(),
                            blue = Random.nextFloat(),
                            alpha = 1f
                        )
                    )
                )
            }
            particles = newParticles

            // Animation loop
            val startTime = System.currentTimeMillis()
            var lastFrameTime = startTime
            while (particles.any { it.isAlive(System.currentTimeMillis()) }) {
                val currentTime = System.currentTimeMillis()
                val deltaTimeS = (currentTime - lastFrameTime) / 1000f
                lastFrameTime = currentTime

                particles = particles.mapNotNull { particle ->
                    if (particle.isAlive(currentTime)) {
                        particle.apply { update(deltaTimeS, 0f, 0f) } // Pass canvas dimensions later
                    } else {
                        null
                    }
                }
                delay(16) // Aim for ~60 FPS
            }
            animationRunning.value = false
            onFinished()
        }
    }

    Canvas(modifier = modifier.fillMaxSize()) {
        val canvasWidth = size.width
        val canvasHeight = size.height

        // Update initial positions if not already centered (first draw)
        // This is a bit of a workaround because initial positions need canvas size.
        if (particles.isNotEmpty() && particles.all { it.x == 0f && it.y == 0f }) {
            particles = particles.map { it.copy(x = canvasWidth / 2, y = canvasHeight / 2) }
        }
        
        particles.forEach { particle ->
            if (particle.isAlive(System.currentTimeMillis())) {
                // Update particle with correct canvas dimensions for boundary checks if any
                 // particle.update(deltaTimeS, canvasWidth, canvasHeight) // This update is now in LaunchedEffect

                drawRect(
                    color = particle.color.copy(alpha = particle.currentAlpha),
                    topLeft = Offset(particle.x - particle.size / 2, particle.y - particle.size / 2),
                    size = androidx.compose.ui.geometry.Size(particle.size, particle.size)
                    // You could also use drawCircle or drawPath for different shapes
                    // For rotation, you'd use withTransform
                )
            }
        }
    }
}
