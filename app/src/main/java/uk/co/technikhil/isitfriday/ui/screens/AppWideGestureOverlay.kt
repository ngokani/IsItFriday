package uk.co.technikhil.isitfriday.ui.screens

import android.content.Context
import android.widget.Toast
import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.awaitEachGesture
import androidx.compose.foundation.gestures.awaitFirstDown
import androidx.compose.foundation.gestures.waitForUpOrCancellation
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.input.pointer.PointerInputScope
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.platform.LocalContext
import androidx.navigation.NavHostController
import androidx.navigation.compose.rememberNavController

@Composable
fun AppWideGestureOverlay(
    modifier: Modifier = Modifier,
    navHostController: NavHostController = rememberNavController()
) {
    val context = LocalContext.current
    Box(
        modifier = modifier
            .fillMaxSize()
            .background(Color.Transparent)
            .pointerInput(Unit) {
                onGesture(context, navHostController)
            }
    )
}

private suspend fun PointerInputScope.onGesture(
    context: Context,
    navHostController: NavHostController
) {
    awaitEachGesture {
        awaitFirstDown().also {
            Toast.makeText(context, "Pointer down", Toast.LENGTH_SHORT)
                .show()
            navHostController.navigate("timer")
        }
        val up = waitForUpOrCancellation()
        if (up != null) {
            Toast.makeText(context, "Pointer up", Toast.LENGTH_SHORT)
                .show()
            navHostController.popBackStack()
        }
    }
}