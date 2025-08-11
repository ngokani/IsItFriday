package uk.co.technikhil.isitfriday.ui.usecases

import java.time.DayOfWeek
import java.time.LocalDate
import javax.inject.Inject

class IsTodayFridayUseCase @Inject constructor() {
    operator fun invoke(): Boolean = LocalDate.now().dayOfWeek == DayOfWeek.FRIDAY
}
