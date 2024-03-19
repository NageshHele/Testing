#ifndef SETTING_HPP
#define SETTING_HPP

#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/ini_parser.hpp>

class Setting
{
    public:
        Setting();
        virtual ~Setting();

        static Setting* getInstance();

        template<typename T>
        const T getValue(const std::string str)
        {
            return pt.get<T>(str);
        }

    protected:

    private:

        static Setting *setting;

        boost::property_tree::ptree pt;
};

#endif // SETTING_HPP
