package uk.co.technikhil.isitfriday.ui.viewmodels

sealed interface AnswerViewIntent {
    data object ViewCreated : AnswerViewIntent
    data object Refresh : AnswerViewIntent
}