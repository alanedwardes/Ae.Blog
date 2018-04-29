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
					echo env.AWS_ACCESS_KEY_ID
					echo env.AWS_SECRET_ACCESS_KEY
					sh "dotnet lambda deploy-serverless" +
					   " --disable-interactive True" +
					   " --stack-name AeBlog" +
					   " --s3-bucket ae-temp"
				}
			}
		}
	}
}