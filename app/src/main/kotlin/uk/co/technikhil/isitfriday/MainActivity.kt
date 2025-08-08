package uk.co.technikhil.isitfriday

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Scaffold
import androidx.compose.ui.Modifier
import androidx.navigation.NavHostController
import androidx.navigation.compose.rememberNavController
import dagger.hilt.android.AndroidEntryPoint
import uk.co.technikhil.isitfriday.ui.navigation.AppNavHost
import uk.co.technikhil.isitfriday.ui.screens.AppWideGestureOverlay
import uk.co.technikhil.isitfriday.ui.theme.IsItFridayTheme

@AndroidEntryPoint
class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            val navController: NavHostController = rememberNavController()
            IsItFridayTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    AppWideGestureOverlay(
                        Modifier.padding(innerPadding),
                        navHostController = navController
                    )
                    AppNavHost(
                        modifier = Modifier.padding(innerPadding),
                        navController = navController,
                    )
                }
            }
        }
    }
}