package uk.co.technikhil.isitfriday.ui.usecases

import java.util.Calendar
import javax.inject.Inject

class IsTodayFridayUseCase @Inject constructor() {
    operator fun invoke(): Boolean {
        val calendar = Calendar.getInstance()
        val dayOfWeek = calendar.get(Calendar.DAY_OF_WEEK)
        return dayOfWeek == Calendar.FRIDAY
    }
}