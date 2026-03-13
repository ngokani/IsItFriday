package uk.co.technikhil.isitfriday.ui.components

import androidx.compose.foundation.Canvas
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.drawscope.rotate
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.unit.dp
import kotlinx.coroutines.isActive
import kotlin.random.Random

data class Particle(
    var x: Float,
    var y: Float,
    var vx: Float,
    var vy: Float,
    var rotation: Float,
    var rotationSpeed: Float,
    val color: Color,
    val size: Float
)

@Composable
fun Confetti(trigger: Int, modifier: Modifier = Modifier) {
    val configuration = LocalConfiguration.current
    val density = LocalDensity.current

    val screenWidthPx = with(density) { configuration.screenWidthDp.dp.toPx() }
    val screenHeightPx = with(density) { configuration.screenHeightDp.dp.toPx() }

    val particles = remember { mutableStateListOf<Particle>() }
    val colors = listOf(Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Magenta, Color.Cyan)

    var frameTimestamp by remember { mutableLongStateOf(0L) }

    LaunchedEffect(trigger) {
        if (trigger == 0) return@LaunchedEffect
        val newParticles = List(75) {
            Particle(
                x = Random.nextFloat() * screenWidthPx,
                y = -50f - Random.nextFloat() * 100f,
                vx = Random.nextFloat() * 400f - 200f,
                vy = Random.nextFloat() * 600f + 400f,
                rotation = Random.nextFloat() * 360f,
                rotationSpeed = Random.nextFloat() * 400f - 200f,
                color = colors.random(),
                size = Random.nextFloat() * 20f + 15f
            )
        }
        particles.addAll(newParticles)
    }

    LaunchedEffect(Unit) {
        var lastFrameTime = withFrameNanos { it }
        while (isActive) {
            withFrameNanos { frameTimeNanos ->
                val dt = (frameTimeNanos - lastFrameTime) / 1_000_000_000f
                lastFrameTime = frameTimeNanos

                val iterator = particles.iterator()
                while (iterator.hasNext()) {
                    val p = iterator.next()
                    p.x += p.vx * dt
                    p.y += p.vy * dt
                    p.rotation += p.rotationSpeed * dt

                    if (p.y > screenHeightPx + p.size) {
                        iterator.remove()
                    }
                }
                frameTimestamp = frameTimeNanos
            }
        }
    }

    Canvas(modifier = modifier.fillMaxSize()) {
        val _tick = frameTimestamp

        particles.toList().forEach { p ->
            rotate(p.rotation, Offset(p.x, p.y)) {
                drawRect(
                    color = p.color,
                    topLeft = Offset(p.x - p.size / 2, p.y - p.size / 2),
                    size = Size(p.size, p.size)
                )
            }
        }
    }
}
