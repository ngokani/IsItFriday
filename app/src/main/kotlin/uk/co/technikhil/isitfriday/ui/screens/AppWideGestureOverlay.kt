package uk.co.technikhil.isitfriday.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.awaitEachGesture
import androidx.compose.foundation.gestures.awaitFirstDown
import androidx.compose.foundation.gestures.awaitLongPressOrCancellation
import androidx.compose.foundation.gestures.waitForUpOrCancellation
// import androidx.compose.foundation.gestures.waitForUpOrCancellation // No longer explicitly used here
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
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
    Box(
        modifier = modifier
            .fillMaxSize()
            .background(Color.Transparent)
            .pointerInput(Unit) { // Pass onTap to the gesture handler
                onGesture(navHostController, onTap)
            }
    )
}

private suspend fun PointerInputScope.onGesture(
    navHostController: NavHostController,
    onTap: () -> Unit// Added onTap callback parameter
) {
    awaitEachGesture {
        // Wait for an unconsumed down event.
        val down = awaitFirstDown(requireUnconsumed = true)

        // `awaitLongPressOrCancellation` will:
        // - Return a `PointerInputChange` if a long press occurs (this event is consumed by it).
        // - Return `null` if the press is released or cancelled before timeout (the up/cancel event is consumed by it).
        val longPressResult = awaitLongPressOrCancellation(down.id)

        if (longPressResult != null) {
            // Long press occurred. `longPressResult` is the consumed long press event.
            // We've decided to handle it, so consume the initial `down` as well.
            down.consume()
            navHostController.navigate("timer")
            val up = waitForUpOrCancellation()
            if (up != null) {
                navHostController.popBackStack()
            }
        } else {
            // No long press. This means the press was shorter (a tap) or was cancelled
            // before the long press timeout. The up/cancel event that terminated
            // `awaitLongPressOrCancellation` is consumed by it.
            // We'll treat this as a tap. Consume the initial `down` event.
            down.consume()
            onTap() // Execute the tap action
        }

        // The original code had a `waitForUpOrCancellation` and `popBackStack()` here.
        // This was removed because:
        // 1. For long presses, it would pop the "timer" screen immediately after navigation.
        // 2. For taps, the `onTap` callback now provides a more explicit way to handle tap actions,
        //    including navigation if desired (e.g., calling `navHostController.popBackStack()` within onTap).
        // 3. The up event that completes a tap or cancels a long press attempt is already
        //    handled by `awaitLongPressOrCancellation`.


    }
}
