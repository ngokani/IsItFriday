package uk.co.technikhil.isitfriday.ui.viewmodels

import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.currentCoroutineContext
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import kotlinx.coroutines.flow.launchIn
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.isActive
import uk.co.technikhil.isitfriday.ui.usecases.IsTodayFridayUseCase
import java.time.Duration
import java.time.LocalDate
import java.time.LocalDateTime
import javax.inject.Inject

@HiltViewModel
class AnswerViewModel @Inject constructor(
    private val isTodayFridayUseCase: IsTodayFridayUseCase,
) : ViewModel() {

    private val _answer = mutableStateOf(false)
    val answer: State<Boolean> = _answer

    fun onIntent(event: AnswerViewIntent) {
        when (event) {
            AnswerViewIntent.ViewCreated -> onViewCreated()
            AnswerViewIntent.Refresh -> onRefresh()
        }
    }

    private fun onViewCreated() {
        onRefresh()
        startDailyRefreshTimer()
    }

    private fun onRefresh() {
        _answer.value = isTodayFridayUseCase()
    }

    private fun startDailyRefreshTimer() {
        midnightTickerFlow
            .onEach { onRefresh() }
            .flowOn(Dispatchers.IO)
            .launchIn(viewModelScope)
    }

    private val midnightTickerFlow =
        flow {
            while (currentCoroutineContext().isActive) {
                val tomorrow = LocalDate.now().plusDays(1)
                val midnight = tomorrow.atStartOfDay()
                val delayUntilMidnight = Duration.between(LocalDateTime.now(), midnight).toMillis()
                delay(delayUntilMidnight)
                emit(Unit)
            }
        }
}

