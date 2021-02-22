package de.p4ddy.facezoombot.config

import com.uchuhimo.konf.ConfigSpec

object OpenCVSpec : ConfigSpec("OpenCV") {
    val faceCascadeClassifierPath by required<String>()
}