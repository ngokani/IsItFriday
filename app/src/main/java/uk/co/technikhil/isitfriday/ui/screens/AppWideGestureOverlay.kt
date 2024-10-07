package uk.co.technikhil.isitfriday.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.awaitEachGesture
import androidx.compose.foundation.gestures.awaitFirstDown
import androidx.compose.foundation.gestures.awaitLongPressOrCancellation
import androidx.compose.foundation.gestures.waitForUpOrCancellation
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
    navHostController: NavHostController = rememberNavController()
) {
    Box(
        modifier = modifier
            .fillMaxSize()
            .background(Color.Transparent)
            .pointerInput(Unit) {
                onGesture(navHostController)
            }
    )
}

private suspend fun PointerInputScope.onGesture(
    navHostController: NavHostController
) {
    awaitEachGesture {
        val down = awaitFirstDown()
        val longPress = awaitLongPressOrCancellation(down.id)
        if (longPress != null) {
            navHostController.navigate("timer")
        }
        val up = waitForUpOrCancellation()
        if (up != null) {
            navHostController.popBackStack()
        }
    }
}