require 'json'

package = JSON.parse(File.read(File.join(__dir__, 'package.json')))

Pod::Spec.new do |s|
  s.name         = "RNUnity"
  s.version      = package['version']
  s.summary      = package['description']
  s.license      = package['license']
  s.homepage     = package['homepage']
  s.authors      = package['author']
  s.platform     = :ios, "10.0"
  s.source       = { :git => package['repository']['url'], :tag => "master" }
  s.source_files  = "ios/RNUnity/**/*.{h,m}"
  s.requires_arc = true

  s.xcconfig = { 'FRAMEWORK_SEARCH_PATHS' => '$(inherited) $(BUILD_ROOT)/** $(CONFIGURATION_BUILD_DIR)/../**' }

  s.dependency "React"

end

  