package de.p4ddy.facezoombot.facezoom

import de.p4ddy.facezoombot.config.ConfigProvider
import de.p4ddy.facezoombot.config.OpenCVSpec
import org.opencv.core.Mat
import org.opencv.core.MatOfByte
import org.opencv.core.MatOfRect
import org.opencv.core.Size
import org.opencv.imgcodecs.Imgcodecs
import org.opencv.imgproc.Imgproc
import org.opencv.objdetect.CascadeClassifier
import org.opencv.objdetect.Objdetect

class FaceZoomer(val configProvider: ConfigProvider) {
    private val faceCascade = CascadeClassifier()

    init {
        faceCascade.load(configProvider.config[OpenCVSpec.faceCascadeClassifierPath])
    }

    fun zoom(picture: ByteArray): List<ByteArray> {
        val image = Imgcodecs.imdecode(MatOfByte(*picture), Imgcodecs.IMREAD_UNCHANGED)
        val grayFrame = Mat()

        val faces = MatOfRect()

        Imgproc.cvtColor(image, grayFrame, Imgproc.COLOR_BGR2GRAY)
        this.faceCascade.detectMultiScale(grayFrame, faces, 1.1, 2, 0 or Objdetect.CASCADE_SCALE_IMAGE)

        val faceList = mutableListOf<ByteArray>()

        faces.toList().forEach { rect ->
            val croppedImage = Mat(image, rect)
            val scaledImage = Mat()
            Imgproc.resize(croppedImage, scaledImage, Size(700.0,700.0))
            val buffer = MatOfByte()
            Imgcodecs.imencode(".png", scaledImage, buffer)
            faceList.add(buffer.toArray())
        }

        return faceList
    }
}