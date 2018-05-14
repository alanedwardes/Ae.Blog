node("linux") {
	timestamps {
		stage ("Checkout") {
			checkout scm
		}

		stage ("Restore") {
			sh "dotnet restore"
		}

		stage ("Deploy") {
			dir ("src/AeBlog") {
				withCredentials([
					[$class: 'AmazonWebServicesCredentialsBinding', credentialsId: 'JenkinsAeBlog']
				]) {
					sh "dotnet lambda deploy-serverless"
				}
			}
		}
	}
}