package uk.co.technikhil.isitfriday.ui.screens

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavHostController
import androidx.navigation.compose.rememberNavController
import uk.co.technikhil.isitfriday.ui.viewmodels.AnswerViewModel

@Composable
fun Answer(
    modifier: Modifier = Modifier,
    navHostController: NavHostController = rememberNavController()
) {
    val viewModel = viewModel<AnswerViewModel>()
    val answerState by viewModel.answer

    Column(
        modifier = modifier.fillMaxSize(),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = if (answerState) "It's Friday" else "It's not Friday"
        )
    }
}