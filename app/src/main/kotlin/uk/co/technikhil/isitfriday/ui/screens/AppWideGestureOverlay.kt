package uk.co.technikhil.isitfriday.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.awaitEachGesture
import androidx.compose.foundation.gestures.awaitFirstDown
import androidx.compose.foundation.gestures.awaitLongPressOrCancellation
import androidx.compose.foundation.gestures.waitForUpOrCancellation
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.input.pointer.PointerInputScope
import androidx.compose.ui.input.pointer.pointerInput
import androidx.navigation.NavHostController
import androidx.navigation.compose.rememberNavController

@Composable
fun AppWideGestureOverlay(
    modifier: Modifier = Modifier,
    navHostController: NavHostController = rememberNavController(),
    onTap: () -> Unit = { } // Added onTap callback
) {
    var showConfetti by remember { mutableStateOf(false) }

    Box(
        modifier = modifier
            .fillMaxSize()
            .background(Color.Transparent)
            .pointerInput(Unit) { // Pass onTap to the gesture handler
                onGesture(navHostController) {
                    onTap() // Call the original onTap logic
                    showConfetti = true // Trigger confetti
                }
            }
    ) {
        if (showConfetti) {
            ConfettiEffect(
                trigger = true, // Pass the trigger state
                onFinished = {
                    showConfetti = false // Reset for next tap
                }
            )
        }
    }
}

private suspend fun PointerInputScope.onGesture(
    navHostController: NavHostController,
    onTap: () -> Unit// Added onTap callback parameter
) {
    awaitEachGesture {
        val down = awaitFirstDown(requireUnconsumed = true)
        val longPressResult = awaitLongPressOrCancellation(down.id)

        if (longPressResult != null) {
            down.consume()
            navHostController.navigate("timer")
            val up = waitForUpOrCancellation()
            if (up != null) {
                navHostController.popBackStack()
            }
        } else {
            down.consume()
            onTap() // Execute the tap action, which now also triggers confetti
        }
    }
}
