package uk.co.technikhil.isitfriday.ui.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
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
    val uiState by viewModel.uiState.collectAsState()

    Column(
        modifier = Modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.background)
            .padding(horizontal = 16.dp)
            .then(modifier)
    ) {
        Spacer(Modifier.weight(1f))
        CountdownText(uiState.timeUntil)
        UntilText(uiState.isItFriday)
        Spacer(Modifier.weight(1f))
    }
}

@Composable
fun UntilText(isItFriday: Boolean) {
    val text = if (isItFriday) {
        stringResource(R.string.until_friday_ends)
    } else {
        stringResource(R.string.until_friday)
    }
    Text(
        text = text,
        style = TextStyle(fontSize = 72.sp),
        color = MaterialTheme.colorScheme.primary,
        modifier = Modifier
            .fillMaxWidth()
    )
}

@Composable
private fun CountdownText(countdownState: TimeUntil) {
    Column(
        modifier = Modifier
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