workflow "Build and deploy on push" {
  on = "push"
  resolves = ["Hello World"]
}

action "Hello World" {
  uses = "./build"
  secrets = ["AWS_ACCESS_KEY", "AWS_SECRET_ACCESS_KEY"]
}
