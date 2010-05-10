class Dir
	def self.exists?(path)
		File.directory?(path)
	end
end

def package_files	
	rm_r PACKAGE_PATH if Dir.exists?(PACKAGE_PATH)
	mkdir PACKAGE_PATH
	copy_third_party_libraries
	copy_conventional_asset "Trains"
end

def copy_third_party_libraries
	files = Dir.glob(File.join("lib/**", "*.dll")) + Dir.glob(File.join("lib/**", "*.pdb"))
	files.each do |item|
		cp_r item, "#{PACKAGE_PATH}"
	end
end

def copy_asset(asset_path, asset_name)
	cp "src\\#{asset_path}\\bin\\#{CONFIG}\\#{asset_name}.dll", "#{PACKAGE_PATH}"
	if #{CONFIG}.downcase == "debug"
		cp "src\\#{asset_path}\\bin\\#{CONFIG}\\#{asset_name}.pdb", "#{PACKAGE_PATH}"
	end
end

def copy_conventional_asset(asset)
		copy_asset "#{asset}", "#{asset}"
		copy_asset "#{asset}.Specs", "#{asset}.Specs"
end