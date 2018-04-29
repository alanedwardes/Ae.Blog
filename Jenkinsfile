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
					env.AWS_REGION = 'eu-west-1'
					sh "dotnet lambda deploy-serverless" +
					   " --disable-interactive True" +
					   " --stack-name AeBlog" +
					   " --s3-bucket ae-temp"
				}
			}
		}
	}
}