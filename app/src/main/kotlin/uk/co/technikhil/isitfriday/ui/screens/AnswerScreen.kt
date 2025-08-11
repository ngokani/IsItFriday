package uk.co.technikhil.isitfriday.ui.screens

import android.content.res.Configuration
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import uk.co.technikhil.isitfriday.R
import uk.co.technikhil.isitfriday.ui.theme.IsItFridayTheme
import uk.co.technikhil.isitfriday.ui.viewmodels.AnswerViewIntent
import uk.co.technikhil.isitfriday.ui.viewmodels.AnswerViewModel

@Composable
fun AnswerScreen(
    modifier: Modifier = Modifier
) {
    val viewModel: AnswerViewModel = hiltViewModel()
    val answerState by viewModel.answer

    LaunchedEffect(key1 = Unit) {
        viewModel.onIntent(AnswerViewIntent.ViewCreated)
    }

    AnswerText(modifier, answerState)
}

@Composable
private fun AnswerText(modifier: Modifier, answerState: Boolean) {
    Column(
        modifier = modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.background)
            .padding(16.dp),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = stringResource(
                id =
                if (answerState) {
                    R.string.yes
                } else {
                    R.string.no
                }
            ),
            style = TextStyle(fontSize = 72.sp),
            color = MaterialTheme.colorScheme.primary
        )
    }
}

@Preview(showBackground = true, showSystemUi = true)
@Composable
fun AnswerPreviewLightMode() {
    IsItFridayTheme {
        AnswerText(
            modifier = Modifier,
            answerState = true
        )
    }
}

@Preview(showBackground = true, showSystemUi = true, uiMode = Configuration.UI_MODE_NIGHT_YES)
@Composable
fun AnswerPreviewDarkMode() {
    IsItFridayTheme {
        AnswerText(
            modifier = Modifier,
            answerState = true
        )
    }
}