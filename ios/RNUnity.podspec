
Pod::Spec.new do |s|
  s.name         = "RNUnity"
  s.version      = "0.1.0"
  s.summary      = "RNUnity"
  s.description  = <<-DESC
                  RNUnity
                   DESC
  s.homepage     = "https://github.com/author/RNUnity.git"
  s.license      = "MIT"
  # s.license      = { :type => "MIT", :file => "FILE_LICENSE" }
  s.author             = { "author" => "author@domain.cn" }
  s.platform     = :ios, "7.0"
  s.source       = { :git => "https://github.com/author/RNUnity.git", :tag => "master" }
  s.source_files  = "**/*.{h,m}"
  s.requires_arc = true

  s.xcconfig = { 'FRAMEWORK_SEARCH_PATHS' => '$(inherited) $(BUILD_ROOT)/** $(CONFIGURATION_BUILD_DIR)/../**' }

  s.framework ='UnityFramework'

  s.dependency "React"
  #s.dependency "others"

end

  