package uk.co.technikhil.isitfriday.ui.viewmodels

import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import java.util.Calendar

class AnswerViewModel : ViewModel() {
    private val _answer = mutableStateOf(false)
    val answer: State<Boolean> = _answer

    init {
        refreshAnswer()
    }

    private fun refreshAnswer() {
        val dayOfWeek = Calendar.getInstance().get(Calendar.DAY_OF_WEEK)
        _answer.value = dayOfWeek == Calendar.FRIDAY
    }

    fun onEvent(event: AnswerEvent) {
        when (event) {
            is AnswerEvent.Refresh -> refreshAnswer()
        }
    }
}

sealed interface AnswerEvent {
    data object Refresh : AnswerEvent
}