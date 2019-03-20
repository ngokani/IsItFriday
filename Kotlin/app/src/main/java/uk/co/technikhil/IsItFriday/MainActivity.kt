package uk.co.technikhil.IsItFriday

import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import android.os.Vibrator
import android.support.v4.app.FragmentActivity

class MainActivity : FragmentActivity() {

    private var mVibrator : Vibrator? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
    }

}
