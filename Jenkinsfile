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
					sh "dotnet lambda deploy-serverless" +
					   " --disable-interactive True" +
					   " --template serverless.template" +
					   " --region eu-west-1" +
					   " --stack-name AeBlog" +
					   " --configuration Release" +
					   " --s3-bucket ae-temp"
				}
			}
		}
	}
}