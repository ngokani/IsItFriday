package uk.co.technikhil.isitfriday.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import uk.co.technikhil.isitfriday.R
import uk.co.technikhil.isitfriday.ui.viewmodels.CountdownViewModel
import uk.co.technikhil.isitfriday.ui.viewmodels.TimeUntil

@Composable
fun CountdownScreen(
    modifier: Modifier = Modifier
) {
    val viewModel: CountdownViewModel = hiltViewModel()
    val countdownState by viewModel.countdown

    CountdownText(modifier, countdownState)
}

@Composable
private fun CountdownText(modifier: Modifier, countdownState: TimeUntil) {
    Column(
        modifier = modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.background),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        with(countdownState) {
            Text(
                text = stringResource(
                    R.string.days_hours_minutes_seconds,
                    days,
                    hours,
                    minutes,
                    seconds
                ),
                style = TextStyle(fontSize = 72.sp),
                color = MaterialTheme.colorScheme.primary
            )
        }
    }
}