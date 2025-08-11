package uk.co.technikhil.isitfriday.ui.viewmodels

import androidx.compose.runtime.MutableState
import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asSharedFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import java.time.DayOfWeek
import java.time.Duration
import java.time.LocalDateTime
import java.time.LocalTime
import javax.inject.Inject

@HiltViewModel
class CountdownViewModel @Inject constructor() : ViewModel() {
    private val _uiState: MutableStateFlow<CountdownUiState> = MutableStateFlow(CountdownUiState())
    val uiState: StateFlow<CountdownUiState> = _uiState.asStateFlow()

    private val isItFriday
        get() = LocalDateTime.now().dayOfWeek == DayOfWeek.FRIDAY

    init {
        startCountdown()
        viewModelScope.launch(Dispatchers.IO) {
            while (true) {
                startCountdown()

                delay(ONE_SECOND_IN_MILLIS.toLong())
            }
        }
    }

    private fun startCountdown() {
        val now = LocalDateTime.now()
        val target = getNextFriday()
        val duration = Duration.between(now, target)
        _uiState.update { state ->
            state.copy(
                timeUntil = TimeUntil(
                    days = duration.toDays().toInt(),
                    hours = (duration.toHours() % ONE_DAY_IN_HOURS).toInt(),
                    minutes = (duration.toMinutes() % ONE_HOUR_IN_MINUTES).toInt(),
                    seconds = ((duration.toMillis() / ONE_SECOND_IN_MILLIS) % ONE_MINUTE_IN_SECONDS).toInt()
                ),
                isItFriday = isItFriday
            )
        }
    }

    private fun getNextFriday(): LocalDateTime {
        val now = LocalDateTime.now()
        val currentDayOfWeek = now.dayOfWeek.value
        val thursdayInt = DayOfWeek.THURSDAY.value
        val daysUntilFriday = when {
            currentDayOfWeek < thursdayInt -> {
                thursdayInt - currentDayOfWeek
            }

            isItFriday ||
            currentDayOfWeek == thursdayInt -> {
                0 // No days to add, calculate time until end of day
            }

            else -> {
                DAYS_IN_WEEK - currentDayOfWeek + thursdayInt
            }
        }
        return now.plusDays(daysUntilFriday.toLong()).with(LocalTime.of(23, 59, 59))
    }

    private companion object {
        const val DAYS_IN_WEEK = 7
        const val ONE_DAY_IN_HOURS = 24
        const val ONE_HOUR_IN_MINUTES = 60
        const val ONE_MINUTE_IN_SECONDS = 60
        const val ONE_SECOND_IN_MILLIS = 1000
    }
}

data class TimeUntil(
    val days: Int,
    val hours: Int,
    val minutes: Int,
    val seconds: Int
)

sealed interface CountdownEvent {
    data object ViewCreated : CountdownEvent
}