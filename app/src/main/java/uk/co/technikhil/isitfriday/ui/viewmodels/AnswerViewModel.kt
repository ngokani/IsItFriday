package uk.co.technikhil.isitfriday.ui.viewmodels

import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import uk.co.technikhil.isitfriday.ui.usecases.IsTodayFridayUseCase
import java.time.LocalDate
import java.time.LocalDateTime
import java.time.LocalTime
import java.time.temporal.ChronoUnit

class AnswerViewModel : ViewModel() {
    private val _answer = mutableStateOf(false)
    val answer: State<Boolean> = _answer

    private val isTodayFriday = IsTodayFridayUseCase()

    init {
        refreshAnswer()
        startRefreshTimer()
    }

    fun onEvent(event: AnswerEvent) {
        when (event) {
            AnswerEvent.ViewCreated -> TODO()
            AnswerEvent.Refresh -> refreshAnswer()
        }
    }

    private fun refreshAnswer() {
        _answer.value = isTodayFriday()
    }

    private fun startRefreshTimer() {
        viewModelScope.launch(Dispatchers.IO) {
            while (true) {
                val now = LocalDateTime.now()
                val midnight = LocalDate.now().atTime(LocalTime.MIDNIGHT)
                val delayMillis = if (now.isBefore(midnight)) {
                    ChronoUnit.MILLIS.between(now, midnight)
                } else {
                    ChronoUnit.MILLIS.between(now, midnight.plusDays(1))
                }

                delay(delayMillis) // Wait until midnight

                withContext(Dispatchers.Main) {
                    refreshAnswer()
                }
            }
        }
    }
}

sealed interface AnswerEvent {
    data object ViewCreated : AnswerEvent
    data object Refresh : AnswerEvent
}