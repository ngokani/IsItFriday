package uk.co.technikhil.isitfriday.ui.viewmodels

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.launchIn
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.flow.update
import java.time.DayOfWeek
import java.time.Duration
import java.time.LocalDateTime
import java.time.temporal.TemporalAdjusters
import javax.inject.Inject

@HiltViewModel
class CountdownViewModel @Inject constructor() : ViewModel() {

    private val _uiState = MutableStateFlow(CountdownUiState())
    val uiState: StateFlow<CountdownUiState> = _uiState.asStateFlow()

    init {
        tickerFlow()
            .onEach { updateCountdownState() }
            .launchIn(viewModelScope)
    }

    private fun updateCountdownState() {
        val now = LocalDateTime.now()
        val target = getTargetDateTime(now)
        val duration = Duration.between(now, target)

        _uiState.update {
            it.copy(
                countdownDuration = CountdownDuration.from(duration),
                isItFriday = now.dayOfWeek == DayOfWeek.FRIDAY
            )
        }
    }

    private fun getTargetDateTime(now: LocalDateTime): LocalDateTime {
        return if (now.dayOfWeek == DayOfWeek.FRIDAY) {
            // If it is Friday, countdown to the end of the day (start of the next day).
            now.toLocalDate().plusDays(1).atStartOfDay()
        } else {
            // Otherwise, countdown to the start of the next Friday.
            now.toLocalDate().with(TemporalAdjusters.next(DayOfWeek.FRIDAY)).atStartOfDay()
        }
    }

    private fun tickerFlow() = flow {
        while (true) {
            emit(Unit)
            delay(ONE_SECOND_IN_MILLIS)
        }
    }

    private companion object {
        const val ONE_SECOND_IN_MILLIS = 1000L
    }
}
