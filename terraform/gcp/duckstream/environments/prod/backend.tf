terraform {
  backend "remote" {
    organization = "ryushiaok/DuckStream"

    workspaces {
      name = "DuckStream"
    }
  }
}
