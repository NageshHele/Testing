#include <iostream>
#include "../include/Setting.hpp"

Setting *Setting::setting = nullptr;

Setting::Setting()
{
    try
    {
        boost::property_tree::ini_parser::read_ini("Setting/MCXFeed.ini", pt);
    }
    catch(std::exception &e)
    {
        std::cout << "Exception caught while reading setting file. Error " << e.what() << std::endl;
    }
}

Setting::~Setting()
{
    //dtor
}

Setting* Setting::getInstance()
{
    if(setting == nullptr)
        setting = new Setting();

    return setting;
}
