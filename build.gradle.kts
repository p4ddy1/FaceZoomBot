import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    kotlin("jvm") version "1.4.21"
    kotlin("plugin.serialization") version "1.4.21"
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
    implementation("com.uchuhimo:konf:1.0.0")
    implementation("org.bytedeco:javacv-platform:1.5.4")
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-core:1.4.2")
    implementation("org.koin:koin-core:2.2.2")
    implementation("org.jetbrains.kotlin:kotlin-reflect:1.4.21")
    implementation("com.fasterxml.jackson.module:jackson-module-kotlin:2.12.+")
    implementation("org.slf4j:slf4j-api:1.7.5")
    implementation("org.slf4j:slf4j-simple:1.7.5")
    implementation("io.github.microutils:kotlin-logging:1.12.0")
    implementation("com.rabbitmq:amqp-client:5.10.0")
    implementation("com.xenomachina:kotlin-argparser:2.0.7")
    implementation("io.github.kotlin-telegram-bot.kotlin-telegram-bot:dispatcher:6.0.2")
    implementation("org.litote.kmongo:kmongo-async:4.2.4")
    implementation("org.litote.kmongo:kmongo-coroutine:4.2.4")
    testImplementation(kotlin("test-junit"))
}

tasks.test {
    useJUnit()
}

tasks.withType<KotlinCompile> {
    kotlinOptions.jvmTarget = "1.8"
}

application {
    mainClassName = "de.p4ddy.facezoombot.MainKt"
}