package uk.co.technikhil.isitfriday.ui.viewmodels

data class CountdownUiState(
    val timeUntil: TimeUntil = TimeUntil(0, 0, 0, 0),
    val isItFriday: Boolean = false
)