package uk.co.technikhil.isitfriday.ui.components

import android.content.Context
import android.hardware.Sensor
import android.hardware.SensorEvent
import android.hardware.SensorEventListener
import android.hardware.SensorManager
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.drawscope.rotate
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.platform.LocalContext
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
fun Confetti(trigger: Int, modifier: Modifier = Modifier, origin: Offset? = null) {
    val configuration = LocalConfiguration.current
    val density = LocalDensity.current

    val screenWidthPx = with(density) { configuration.screenWidthDp.dp.toPx() }
    val screenHeightPx = with(density) { configuration.screenHeightDp.dp.toPx() }

    val particles = remember { mutableStateListOf<Particle>() }
    val colors = listOf(Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Magenta, Color.Cyan)

    var frameTimestamp by remember { mutableLongStateOf(0L) }
    
    val context = LocalContext.current
    var sensorGravityX by remember { mutableFloatStateOf(0f) }
    var sensorGravityY by remember { mutableFloatStateOf(9.8f) } // Default gravity

    DisposableEffect(context) {
        val sensorManager = context.getSystemService(Context.SENSOR_SERVICE) as SensorManager
        val accelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER)
        
        val listener = object : SensorEventListener {
            override fun onSensorChanged(event: SensorEvent?) {
                if (event != null) {
                    // Accelerometer returns m/s^2.
                    // X axis: Positive is right.
                    // Y axis: Positive is up (so Earth's gravity gives +9.8 when device is flat, 
                    // but since the UI Y-axis is inverted (0 at top), we invert the Y reading).
                    sensorGravityX = -event.values[0] 
                    sensorGravityY = event.values[1]
                }
            }

            override fun onAccuracyChanged(sensor: Sensor?, accuracy: Int) {}
        }

        sensorManager.registerListener(listener, accelerometer, SensorManager.SENSOR_DELAY_GAME)

        onDispose {
            sensorManager.unregisterListener(listener)
        }
    }

    LaunchedEffect(trigger, origin) {
        if (trigger == 0) return@LaunchedEffect
        
        val startX = origin?.x ?: (screenWidthPx / 2f)
        val startY = origin?.y ?: (screenHeightPx / 2f)
        
        val newParticles = List(75) {
            val angle = Random.nextFloat() * 2 * Math.PI
            val speed = Random.nextFloat() * 800f + 200f
            Particle(
                x = startX,
                y = startY,
                vx = (Math.cos(angle) * speed).toFloat(),
                vy = (Math.sin(angle) * speed).toFloat() - 400f, // Initial upward burst
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
                
                // Scale real world gravity to UI pixels
                val uiGravityX = sensorGravityX * 150f
                val uiGravityY = sensorGravityY * 150f

                val iterator = particles.iterator()
                while (iterator.hasNext()) {
                    val p = iterator.next()
                    p.vx += uiGravityX * dt
                    p.vy += uiGravityY * dt
                    
                    p.x += p.vx * dt
                    p.y += p.vy * dt
                    p.rotation += p.rotationSpeed * dt

                    // Add a tiny bit of padding to the screen height bounds so particles fall all the way down
                    if (p.y > screenHeightPx + p.size || p.x < -100f || p.x > screenWidthPx + 100f) {
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
