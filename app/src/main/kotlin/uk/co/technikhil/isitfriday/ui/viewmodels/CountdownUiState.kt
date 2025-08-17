package uk.co.technikhil.isitfriday.ui.viewmodels

data class CountdownUiState(
    val countdownDuration: CountdownDuration = CountdownDuration(0, 0, 0, 0),
    val isItFriday: Boolean = false
)