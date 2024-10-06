package uk.co.technikhil.isitfriday.ui.navigation

import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import uk.co.technikhil.isitfriday.ui.screens.AnswerScreen
import uk.co.technikhil.isitfriday.ui.screens.CountdownScreen

@Composable
fun AppNavHost(
    modifier: Modifier = Modifier,
    navController: NavHostController = rememberNavController(),
    startDestination: String = "home",
) {
    NavHost(
        navController = navController,
        startDestination = startDestination,
        modifier = modifier
    ) {
        composable("home") {
            AnswerScreen(
                modifier = modifier
            )
        }
        composable("timer") {
            CountdownScreen(
                modifier = modifier
            )
        }
    }
}