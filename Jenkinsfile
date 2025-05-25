pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            steps {
                echo 'Building...'
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Publish') {
            steps {
                echo 'Publishing...'
                bat 'dotnet publish -c Release -o "D:\\C# Projects\\BusManagement\\BusManagementAPI\\bin\\Release\\net8.0\\publish"'
            }
        }

        stage('Deploy to IIS') {
            steps {
                echo 'No deploy step needed — IIS reads directly from the publish path.'
            }
        }
    }

    post {
        success {
            echo '✅ Build and publish successful. IIS should serve from the updated path.'
        }
        failure {
            echo '❌ Build or publish failed.'
        }
    }
}
