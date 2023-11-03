# terraform {
#   backend "remote" {
#     organization = "ryushiaok/DuckStream"

#     workspaces {
#       name = "DuckStream"
#     }
#   }
# }

terraform {
  backend "gcs" {
    bucket = "duckstream_tfstate_bucket"
    prefix = "terraform/state"
  }
}
