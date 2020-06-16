
lazy val docs = project
  .in(file("."))
  .enablePlugins(CloudstateParadoxPlugin)
  .settings(
    deployModule := "dotnet",
    paradoxProperties in Compile ++= Map(
      "cloudstate.dotnet.version" -> "2.2",
      "cloudstate.dotnet-support.version" -> { if (isSnapshot.value) previousStableVersion.value.getOrElse("0.0.0") else version.value },
      "extref.cloudstate.base_url" -> "https://cloudstate.io/docs/core/current/%s",
      "snip.base.base_dir" -> s"${(baseDirectory in ThisBuild).value.getAbsolutePath}/../",
    )
  )