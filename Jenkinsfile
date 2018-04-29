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
				sh "dotnet lambda deploy-serverless"
			}
		}
	}
}