package uk.co.technikhil.isitfriday.ui.viewmodels

import java.time.Duration

data class CountdownDuration(
    val days: Int,
    val hours: Int,
    val minutes: Int,
    val seconds: Int
) {
    companion object {
        private const val ONE_DAY_IN_HOURS = 24
        private const val ONE_HOUR_IN_MINUTES = 60
        private const val ONE_MINUTE_IN_SECONDS = 60

        fun from(duration: Duration): CountdownDuration {
            if (duration.isNegative) {
                return CountdownDuration(0, 0, 0, 0)
            }

            return CountdownDuration(
                days = duration.toDays().toInt(),
                hours = (duration.toHours() % ONE_DAY_IN_HOURS).toInt(),
                minutes = (duration.toMinutes() % ONE_HOUR_IN_MINUTES).toInt(),
                seconds = (duration.seconds % ONE_MINUTE_IN_SECONDS).toInt()
            )
        }
    }
}
