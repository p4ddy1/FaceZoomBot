package de.p4ddy.facezoombot.telegram.api.exception

import com.github.kotlintelegrambot.network.ResponseError

class RequestFailedException(message: String) : Exception(message) {
    companion object Factory {
        fun fromResponseError(responseError: ResponseError): RequestFailedException {
            if (responseError.exception != null) {
                return RequestFailedException(responseError.exception!!.message.orEmpty())
            }

            if (responseError.errorBody != null) {
                return RequestFailedException(responseError.errorBody!!.string())
            }

            return RequestFailedException("Unknown error")
        }
    }
}