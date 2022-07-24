import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    kotlin("jvm") version "1.7.10"
    kotlin("plugin.serialization") version "1.7.10"
    id("org.bytedeco.gradle-javacpp-platform") version "1.5.4"
    application
}

group = "de.p4ddy"
version = "1.0-SNAPSHOT"

repositories {
    maven("https://jitpack.io")
    mavenCentral()
    jcenter()
}

dependencies {
    implementation("com.uchuhimo:konf:1.1.2")
    implementation("org.bytedeco:opencv-platform:4.5.5-1.5.7")
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-core:1.6.2")
    implementation("org.koin:koin-core:2.2.2")
    implementation("org.jetbrains.kotlin:kotlin-reflect:1.7.10")
    implementation("com.fasterxml.jackson.module:jackson-module-kotlin:2.13.3")
    implementation("org.slf4j:slf4j-api:1.7.36")
    implementation("org.slf4j:slf4j-simple:1.7.36")
    implementation("io.github.microutils:kotlin-logging:2.1.23")
    implementation("com.rabbitmq:amqp-client:5.14.2")
    implementation("com.xenomachina:kotlin-argparser:2.0.7")
    implementation("io.github.kotlin-telegram-bot.kotlin-telegram-bot:telegram:6.0.7")
    implementation("org.litote.kmongo:kmongo-async:4.5.1")
    implementation("org.litote.kmongo:kmongo-coroutine:4.5.1")
    testImplementation(kotlin("test-junit"))
}

tasks.test {
    useJUnit()
}

tasks.withType<KotlinCompile> {
    kotlinOptions.jvmTarget = "1.8"
}

tasks.getByName<Zip>("distZip").enabled = false
tasks.getByName<Tar>("distTar").archiveName = "facezoombot.tar"

application {
    mainClassName = "de.p4ddy.facezoombot.MainKt"
}
